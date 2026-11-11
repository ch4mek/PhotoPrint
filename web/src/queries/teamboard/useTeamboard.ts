import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_TEAMBOARD_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { ToastContextValues } from '@/context/types';
import { capitalizeString } from '@/helpers/capitalizeString';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { createToast } from '@/helpers/createToast';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { subtractPeriodFromCurrentDate } from '@/helpers/subtractYearsFromCurrentDate';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import dayjs from '@/lib/dayjs';
import { TeamboardFilterType } from '@/pages/Teamboard/types';
import {
	CalendaryCountry,
	EnglishLevelType,
	PersistentConfigurationItem,
	SeniorityLevelType,
} from '@/types/configuration';
import { TeamboardResponse } from '@/types/teamboard';

const paramsToReverseOrder = ['careerStartedAt', 'positionOrderWeight'];

export const useTeamboard = (
	toast: ToastContextValues,
	page: number,
	rowsPerPage: number,
	order: SortDirection,
	orderByCondition: string,
	filters: TeamboardFilterType,
	allLocations: PersistentConfigurationItem[],
	allPositions: PersistentConfigurationItem[],
	allSeniorities: SeniorityLevelType[],
	allEnglishLevel: EnglishLevelType[],
	allRegisteredLocation: CalendaryCountry[],
	allDepartments: Array<SelectionModalOption>
) =>
	useQueryWithCallback<TeamboardResponse>({
		queryKey: [
			TeamboardQueriesKeys.teamboard,
			order,
			page,
			rowsPerPage,
			orderByCondition,
			filters,
		],
		queryFn: async () => {
			try {
				const {
					userId,
					user,
					professionalExperience: { from: fromExperience, to: toExperience },
					englishLevel,
					hardSkills,
					specialization,
					currentLocation,
					position,
					location,
					comment,
					departments,
					registeredLocation,
					groups,
				} = filters;

				const idToNameMap = allRegisteredLocation.reduce((acc, obj) => {
					acc[obj.id] = obj.name;

					return acc;
				}, {} as Record<number, string>);

				const registeredLocationCountries = registeredLocation.map(
					(number) => idToNameMap[+number]
				);

				const defineToExperience = (toExp: string | null | undefined) => {
					if (!toExp) {
						return;
					}

					if (toExp === '0') {
						return `date(careerStartedAt) ge ${dayjs(
							subtractPeriodFromCurrentDate('months', 1)
						)
							.add(1, 'days')
							.format(YYYY_MM_DD_DAYJS_DATE_FORMAT)}`;
					}

					return `date(careerStartedAt) ge ${subtractPeriodFromCurrentDate(
						'years',
						parseInt(toExp, 10)
					)}`;
				};

				const query = buildQuery({
					filter: {
						and: [
							userId &&
								`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
									userId
								)}', Edm.String)`,
							user &&
								`contains(tolower(fullName), tolower('${replaceSpecialCharacters(
									user
								)}'))`,

							defineToExperience(toExperience),
							fromExperience &&
								`date(careerStartedAt) le ${subtractPeriodFromCurrentDate(
									'years',
									parseInt(fromExperience, 10)
								)}`,

							comment &&
								`contains(tolower(comments), tolower('${replaceSpecialCharacters(
									comment
								)}'))`,

							currentLocation &&
								`contains(tolower(currentLocation), tolower('${replaceSpecialCharacters(
									currentLocation
								)}'))`,

							groups &&
								`contains(tolower(resourceGroupName), tolower('${replaceSpecialCharacters(
									groups
								)}'))`,

							specialization && {
								or: [
									`odpNonPrimarySpecializations/any(s: contains(tolower(s/name), tolower('${replaceSpecialCharacters(
										specialization
									)}')))`,
									{
										'tolower(primarySpecializationName)': {
											contains:
												replaceSpecialCharacters(specialization).toLowerCase(),
										},
									},
								],
							},
							{
								registeredLocation: convertFilterArrayToQueryParam(
									registeredLocationCountries,
									allRegisteredLocation.length
								),
							},
							{
								englishLevelId: convertFilterArrayToQueryParam(
									englishLevel,
									allEnglishLevel.length
								),
							},
							{
								departmentId: convertFilterArrayToQueryParam(
									departments,
									allDepartments.length
								),
							},
							{
								seniorityLevelId: convertFilterArrayToQueryParam(
									hardSkills,
									allSeniorities.length
								),
							},
							{
								position: convertFilterArrayToQueryParam(
									position,
									allPositions.length
								),
							},
							{
								location: convertFilterArrayToQueryParam(
									location,
									allLocations.length
								),
							},
						],
					},

					orderBy: `${capitalizeString(
						orderByCondition
					)} ${convertOrderToQueryParam(
						order,
						orderByCondition,
						paramsToReverseOrder
					)}`,
					top: rowsPerPage,
					skip: (page - 1) * rowsPerPage,
				});

				const res = await axios.get<TeamboardResponse>(
					`${GET_TEAMBOARD_API_ENDPOINT}/${query}`
				);

				return res.data;
			} catch (error) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						(error as any).response.data.Message ?? SOMETHING_WENT_WRONG
					)
				);

				return Promise.reject(error);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
