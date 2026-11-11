import { useMutation } from '@tanstack/react-query';
import axios from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { GET_EQUIPMENT_HISTORY_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

type DeleteLastEquipmentHistoryItemArgs = {
	equipmentId: string;
	equipmentHistoryId: number;
};

export const useDeleteLastEquipmentHistoryItem = (toast: ToastContextValues) =>
	useMutation({
		mutationFn: async ({
			equipmentId,
			equipmentHistoryId,
		}: DeleteLastEquipmentHistoryItemArgs) => {
			try {
				const { data } = await axios.delete(
					`${GET_EQUIPMENT_HISTORY_API_ENDPOINT}/${equipmentId}/history/${equipmentHistoryId}`
				);

				return data;
			} catch {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));

				return Promise.reject();
			}
		},
	});
