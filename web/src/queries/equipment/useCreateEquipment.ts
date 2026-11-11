import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	NotificationType,
	EQUIPMENT_CREATED,
	SAME_SERIAL_NUMBER_ALREADY_EXISTS,
	PLEASE_CHANGE_SERIAL_NUMBER,
} from '@/constants/notifications';
import { GET_EQUIPMENT_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { getDateAsResponseType } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import type { Dayjs } from '@/lib/dayjs';
import { EquipmentItemRequest, EquipmentItemResponse } from '@/types/equipment';

export const useCreateEquipment = (
	toast: ToastContextValues,
	setIsSerialNumberUnique: React.Dispatch<React.SetStateAction<boolean>>,
	options?: UseMutationOptions<
		EquipmentItemResponse,
		AxiosError,
		EquipmentItemRequest
	>
) =>
	useMutation({
		...options,
		mutationFn: async ({
			name,
			equipmentTypeId,
			serialNumber,
			invoiceNumber,
			purchasePrice,
			purchaseCurrency,
			purchasePriceUsd,
			purchaseDate,
			purchasePlace,
			guaranteeDate,
			characteristics,
			approverId,
			location,
			comment,
			activeHolderRequest,
			brokenStatus,
			jiraTask,
			storage,
			repairPlace,
		}) => {
			try {
				const { data } = await axios.post(GET_EQUIPMENT_API_ENDPOINT, {
					name,
					equipmentTypeId,
					serialNumber,
					invoiceNumber,
					purchasePrice: Number(purchasePrice),
					purchasePriceUsd,
					purchaseDate: getDateAsResponseType(purchaseDate as Dayjs),
					purchasePlace,
					guaranteeDate: getDateAsResponseType(guaranteeDate as Dayjs),
					characteristics,
					approverId,
					location,
					comment: comment || null,
					activeHolder: activeHolderRequest,
					purchaseCurrency,
					brokenStatus,
					jiraTask,
					storage,
					repairPlace,
				});
				toast.open(createToast(NotificationType.SUCCESS, EQUIPMENT_CREATED));

				return data;
			} catch (error) {
				if (
					(error as any).response.data.Message ===
					SAME_SERIAL_NUMBER_ALREADY_EXISTS
				) {
					toast.open(
						createToast(NotificationType.ERROR, PLEASE_CHANGE_SERIAL_NUMBER)
					);
					setIsSerialNumberUnique(false);
				} else {
					toast.open(
						createToast(
							NotificationType.ERROR,
							(error as any).response.data.Message ?? SOMETHING_WENT_WRONG
						)
					);
				}

				return Promise.reject(error);
			}
		},
	});
