import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { EquipmentQueriesKeys } from '@/constants/queriesKeys';
import { GET_EQUIPMENT_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import type { EquipmentItemResponse } from '@/types/equipment';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetEquipmentById = (
	toast: ToastContextValues,
	id: string,
	options?: PatchedQueryOptions<EquipmentItemResponse, AxiosError>
) =>
	useQueryWithCallback<EquipmentItemResponse, AxiosError>({
		queryKey: [EquipmentQueriesKeys.editing, id],
		...options,
		queryFn: async () => {
			try {
				const { data } = await axios.get<EquipmentItemResponse>(
					`${GET_EQUIPMENT_API_ENDPOINT}/${id}`
				);

				return data;
			} catch (error) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						(error as any).response.data.Message ?? SOMETHING_WENT_WRONG
					)
				);

				return Promise.reject();
			}
		},
		refetchOnWindowFocus: false,
		enabled: Boolean(id),
		refetchOnMount: true,
	});
