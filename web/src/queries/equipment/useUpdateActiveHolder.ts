import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType, EQUIPMENT_UPDATED } from '@/constants/notifications';
import { GET_EQUIPMENT_HISTORY_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { getDateAsResponseType } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import type { EquipmentHistoryItem } from '@/types/equipment';

import type { UpdateActiveHolderVariables } from './types';

export const useUpdateActiveHolder = (
	toast: ToastContextValues,
	options?: UseMutationOptions<
		EquipmentHistoryItem,
		AxiosError,
		UpdateActiveHolderVariables
	>
) =>
	useMutation({
		...options,
		mutationFn: async ({ equipmentId, userId, issueDate, approverId }) => {
			try {
				const { data } = await axios.post(
					`${GET_EQUIPMENT_HISTORY_API_ENDPOINT}/${equipmentId}/assign`,
					{
						userId,
						approverId,
						issueDate: getDateAsResponseType(issueDate),
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
