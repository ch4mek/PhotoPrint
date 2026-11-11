import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils';
import { getLogicFilterValue } from '../utils/getLogicFilterValue';

import { UserEquipmentFilters } from '@/components/UserProfile/components/EquipmentTab/types';
import { AssignStatus } from '@/constants/equipment';
import { EquipmentQueriesKeys } from '@/constants/queriesKeys';
import { getEquipmentByUserId } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { ToastContextValues } from '@/context/types';
import { getCorrectUTCStringFormatFromDate } from '@/helpers/getCorrectUTCStringFormatFromDate';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import {
	createOnErrorHandler,
	handleRequestError,
} from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { EquipmentType } from '@/types/configuration';
import { UserEquipmentResponse } from '@/types/equipment';

export const useGetEquipmentByUserId = (
	userId: number,
	top: number,
	skip: number,
	order: SortDirection,
	orderByCondition: string,
	filters: UserEquipmentFilters,
	allEquipmentTypes: Array<EquipmentType>,
	toast: ToastContextValues,
	isDeletedEquipment?: boolean
) =>
	useQueryWithCallback<UserEquipmentResponse, AxiosError<ServerResponseError>>({
		queryKey: [
			EquipmentQueriesKeys.user,
			userId,
			order,
			orderByCondition,
			filters,
			top,
			skip,
		],
		queryFn: async () => {
			const {
				equipmentName,
				equipmentType,
				serialNumber,
				inventoryNumber,
				issueDate,
				returnDate,
				isAssigned,
			} = filters;

			try {
				const searchFilter = {
					and: [
						`isDeleted eq ${Boolean(isDeletedEquipment)}`,
						equipmentName &&
							`contains(tolower(equipmentName), tolower('${replaceSpecialCharacters(
								equipmentName
							)}'))`,
						isAssigned.length &&
							getLogicFilterValue(
								isAssigned,
								AssignStatus.Assigned,
								'isAssigned'
							),
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
					top,
					skip: skip * top,
					orderBy: `${orderByCondition} ${order}`,
					filter: {
						and: [
							{
								equipmentTypeId: convertFilterArrayToQueryParam(
									equipmentType,
									allEquipmentTypes.length
								),
							},
							searchFilter,
						],
					},
				});

				const response = await axios.get<UserEquipmentResponse>(
					`${getEquipmentByUserId(userId)}${query}`
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		onError: createOnErrorHandler(toast),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
