import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType, EQUIPMENT_UPDATED } from '@/constants/notifications';
import { GET_EQUIPMENT_HISTORY_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { getDateAsResponseType } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import type { EquipmentHistoryItem } from '@/types/equipment';

import type { DeleteActiveHolderVariables } from './types';

export const useDeleteActiveHolder = (
	toast: ToastContextValues,
	options?: UseMutationOptions<
		EquipmentHistoryItem,
		AxiosError,
		DeleteActiveHolderVariables
	>
) =>
	useMutation({
		...options,
		mutationFn: async ({ equipmentId, returnDate }) => {
			try {
				const { data } = await axios.post(
					`${GET_EQUIPMENT_HISTORY_API_ENDPOINT}/${equipmentId}/return`,
					{
						returnDate: getDateAsResponseType(returnDate),
					}
				);
				toast.open(createToast(NotificationType.SUCCESS, EQUIPMENT_UPDATED));

				return data;
			} catch (error) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						(error as any).response.data.Message
					)
				);

				return Promise.reject(error);
			}
		},
	});
