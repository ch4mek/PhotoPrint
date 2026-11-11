import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';
import { getLogicFilterValue } from '../utils/getLogicFilterValue';

import { ActivityStatus } from '@/constants/activityStatus';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { LabboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_LABBOARD_INTERNS_API_ENDPOINT } from '@/constants/queryPaths';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { labboardOrderByToODataPath } from '@/pages/Labboard/constants';
import { LabboardInternsResponse } from '@/types/labboard';

import { UseGetLabboardInternsParams } from './types';

const paramsToReverseOrder = ['isActive'];

export const useGetLabboardInterns = ({
	page,
	rowsPerPage,
	order,
	orderBy,
	filters,
	englishLevelsOptionsLength,
	internshipTypesOptionsLength,
	finalResultsOptionsLength,
	options,
}: UseGetLabboardInternsParams) => {
	const {
		id,
		user,
		internshipSpecialization,
		group,
		internshipType,
		internshipEngineerStage,
		englishLevel,
		internshipApprovedPoints,
		internshipCompletedPoints,
		currentLocation,
		internshipStartDate,
		internshipPlannedEndDate,
		status,
		internshipFinalResult,
		comments,
	} = filters;

	return useQueryWithCallback<
		LabboardInternsResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			LabboardQueriesKeys.labboardInterns,
			page,
			rowsPerPage,
			order,
			orderBy,
			filters,
		],
		queryFn: async () => {
			const groupFilter =
				group.trim() === NO_VALUE_FILTER_OPTION.name
					? 'resourceGroupName eq null'
					: group.trim() &&
					  `contains(tolower(resourceGroupName), tolower('${replaceSpecialCharacters(
							group.trim()
					  )}'))`;

			const query = buildQuery({
				filter: {
					and: [
						id?.trim() &&
							`cast(id, Edm.String) eq cast('${replaceSpecialCharacters(
								id.trim()
							)}', Edm.String)`,
						user?.trim() &&
							`contains(tolower(fullName), tolower('${replaceSpecialCharacters(
								user.trim()
							)}'))`,
						internshipSpecialization?.trim() &&
							`contains(tolower(internship/specialization), tolower('${replaceSpecialCharacters(
								internshipSpecialization.trim()
							)}'))`,
						groupFilter,
						internshipType?.length && {
							'internship/type': convertFilterArrayToQueryParam(
								internshipType,
								internshipTypesOptionsLength
							),
						},
						internshipEngineerStage?.trim() &&
							`cast(internship/engineerStage, Edm.String) eq cast('${replaceSpecialCharacters(
								internshipEngineerStage.trim()
							)}', Edm.String)`,
						{
							englishLevelId: convertFilterArrayToQueryParam(
								englishLevel,
								englishLevelsOptionsLength
							),
						},
						internshipApprovedPoints?.from &&
							`internship/approvedPoints ge ${Number(
								internshipApprovedPoints.from
							)}`,
						internshipApprovedPoints?.to &&
							`internship/approvedPoints le ${Number(
								internshipApprovedPoints.to
							)}`,
						internshipCompletedPoints?.from &&
							`internship/completedPoints ge ${Number(
								internshipCompletedPoints.from
							)}`,
						internshipCompletedPoints?.to &&
							`internship/completedPoints le ${Number(
								internshipCompletedPoints.to
							)}`,
						currentLocation?.trim() &&
							`contains(tolower(currentLocation), tolower('${replaceSpecialCharacters(
								currentLocation.trim()
							)}'))`,
						...convertDateFromToValueToQueryParam(
							internshipStartDate,
							'internship/startDate'
						),
						...convertDateFromToValueToQueryParam(
							internshipPlannedEndDate,
							'internship/plannedEndDate'
						),
						status &&
							getLogicFilterValue(status, ActivityStatus.Active, 'isActive'),
						{
							'internship/finalResult': convertFilterArrayToQueryParam(
								internshipFinalResult,
								finalResultsOptionsLength
							),
						},
						comments?.trim() &&
							`contains(tolower(comments), tolower('${replaceSpecialCharacters(
								comments.trim()
							)}'))`,
					],
				},
				orderBy: `${
					labboardOrderByToODataPath[orderBy] ?? orderBy
				} ${convertOrderToQueryParam(order, orderBy, paramsToReverseOrder)}`,
				top: rowsPerPage,
				skip: (page - 1) * rowsPerPage,
			});

			const { data } = await axios.get<LabboardInternsResponse>(
				`${GET_LABBOARD_INTERNS_API_ENDPOINT}${query}`
			);

			return data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
};
