import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { EquipmentQueriesKeys } from '@/constants/queriesKeys';
import { GET_EQUIPMENT_HISTORY_API_ENDPOINT } from '@/constants/queryPaths';
import { SortDirection } from '@/constants/sort';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { EquipmentHistoryResponse } from '@/types/equipment';

import { UseGetEquipmentHistoryParams } from './types';

const EQUIPMENT_HISTORY_DESC_FILTER =
	'issueDate desc,returnDate desc,equipmentHistoryId desc';
const EQUIPMENT_HISTORY_ASC_FILTER =
	'issueDate asc,returnDate asc,equipmentHistoryId asc';

export const useGetEquipmentHistory = ({
	toast,
	id,
	page,
	rowsPerPage,
	isDeletedFilter,
	orderBy,
	enabled = true,
}: UseGetEquipmentHistoryParams) =>
	useQueryWithCallback({
		queryKey: [
			EquipmentQueriesKeys.history,
			id,
			page,
			rowsPerPage,
			isDeletedFilter,
			orderBy,
		],
		queryFn: async () => {
			const orderFilter =
				orderBy === SortDirection.DESC
					? EQUIPMENT_HISTORY_DESC_FILTER
					: EQUIPMENT_HISTORY_ASC_FILTER;
			try {
				const query = buildQuery({
					top: rowsPerPage,
					skip: (page - 1) * rowsPerPage,
					orderBy: orderFilter,
					filter: {
						and: [
							{
								and: [
									isDeletedFilter !== undefined &&
										`isDeleted eq ${isDeletedFilter}`,
								],
							},
						],
					},
				});
				const { data } = await axios.get<EquipmentHistoryResponse>(
					`${GET_EQUIPMENT_HISTORY_API_ENDPOINT}/${id}/history${query}`
				);

				return data;
			} catch {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));

				return Promise.reject();
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: Boolean(id) && enabled,
	});
