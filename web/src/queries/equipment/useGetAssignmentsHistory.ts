import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { convertFilterArrayToQueryParam } from '../utils';
import { getLogicFilterValue } from '../utils/getLogicFilterValue';

import { ASSIGNMENTS_HISTORY_QUERY_KEY } from '@/constants/queriesKeys';
import { GET_ASSIGNMENTS_HISTORY_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { convertOrderToQueryParam } from '@/helpers/convertOrderToQueryParam';
import { getCorrectUTCStringFormatFromDate } from '@/helpers/getCorrectUTCStringFormatFromDate';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { AssignmentHistoryFilters } from '@/pages/Equipment/components/AssignmentsHistory/types';
import {
	GetAssignmentsHistoryResponse,
	ServerResponseError,
} from '@/queries/types';
import { DepartmentType, EquipmentType } from '@/types/configuration';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetAssignmentsHistory = (
	skip: number,
	top: number,
	order: SortDirection,
	orderByCondition: string,
	filter: AssignmentHistoryFilters,
	allDepartments: Array<DepartmentType>,
	allEquipmentTypes: Array<EquipmentType>,
	options?: PatchedQueryOptions<
		GetAssignmentsHistoryResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetAssignmentsHistoryResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			ASSIGNMENTS_HISTORY_QUERY_KEY,
			skip,
			top,
			order,
			orderByCondition,
			filter,
		],
		queryFn: async () => {
			const {
				userId,
				equipmentId,
				userFullName,
				isAssigned,
				departments,
				equipmentName,
				type,
				serialNumber,
				inventoryNumber,
				issueDate,
				returnDate,
			} = filter;

			try {
				const searchFilter = {
					and: [
						userFullName &&
							`contains(tolower(userFullName), tolower('${replaceSpecialCharacters(
								userFullName
							)}'))`,
						isAssigned && getLogicFilterValue(isAssigned, 'Yes', 'isAssigned'),
						equipmentName &&
							`contains(tolower(equipmentName), tolower('${replaceSpecialCharacters(
								equipmentName
							)}'))`,
						serialNumber &&
							`contains(tolower(serialNumber), tolower('${replaceSpecialCharacters(
								serialNumber
							)}'))`,
						inventoryNumber &&
							`contains(tolower(inventoryNumber), tolower('${replaceSpecialCharacters(
								inventoryNumber
							)}'))`,
						issueDate?.from &&
							`cast(issueDate, Edm.DateTimeOffset) ge cast(${getCorrectUTCStringFormatFromDate(
								issueDate?.from
							)}, Edm.DateTimeOffset)`,
						issueDate?.to &&
							`cast(issueDate, Edm.DateTimeOffset) le cast(${getCorrectUTCStringFormatFromDate(
								issueDate?.to
							)}, Edm.DateTimeOffset)`,
						returnDate?.from &&
							`cast(returnDate, Edm.DateTimeOffset) ge cast(${getCorrectUTCStringFormatFromDate(
								returnDate?.from
							)}, Edm.DateTimeOffset)`,
						returnDate?.to &&
							`cast(returnDate, Edm.DateTimeOffset) le cast(${getCorrectUTCStringFormatFromDate(
								returnDate?.to
							)}, Edm.DateTimeOffset)`,
					],
				};

				const query = buildQuery({
					filter: {
						and: [
							userId && `userId eq ${userId}`,
							equipmentId && `equipmentId eq ${equipmentId}`,
							{
								equipmentTypeId: convertFilterArrayToQueryParam(
									type,
									allEquipmentTypes.length
								),
							},
							{
								userDepartmentId: convertFilterArrayToQueryParam(
									departments,
									allDepartments.length
								),
							},
							searchFilter,
						],
					},
					orderBy: `${capitalizeString(
						orderByCondition
					)} ${convertOrderToQueryParam(order, orderByCondition, [
						'isAssigned',
					])}`,
					top,
					skip: skip * top,
				});

				const res = await axios.get<GetAssignmentsHistoryResponse>(
					`${GET_ASSIGNMENTS_HISTORY_API_ENDPOINT}${query}`
				);

				return res.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
