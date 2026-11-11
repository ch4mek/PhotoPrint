import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import {
	NotificationType,
	EQUIPMENT_UPDATED,
	SAME_SERIAL_NUMBER_ALREADY_EXISTS,
	PLEASE_CHANGE_SERIAL_NUMBER,
	SAME_INVENTORY_NUMBER_ALREADY_EXISTS,
	PLEASE_CHANGE_INVENTORY_NUMBER,
} from '@/constants/notifications';
import { GET_EQUIPMENT_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { getDateAsResponseType } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import { Dayjs } from '@/lib/dayjs';
import type { EquipmentItemResponse } from '@/types/equipment';

import type { UpdateMutationVariables } from './types';

export const useUpdateEquipment = (
	toast: ToastContextValues,
	setIsSerialNumberUnique: React.Dispatch<React.SetStateAction<boolean>>,
	setIsInventoryNumberUnique: React.Dispatch<React.SetStateAction<boolean>>,
	options?: UseMutationOptions<
		EquipmentItemResponse,
		AxiosError,
		UpdateMutationVariables
	>
) =>
	useMutation({
		...options,
		mutationFn: async ({
			name,
			equipmentTypeId,
			serialNumber,
			inventoryNumber,
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
			equipmentId,
			activeHolderRequest,
			brokenStatus,
			jiraTask,
			storage,
			repairPlace,
		}) => {
			try {
				const { data } = await axios.put(GET_EQUIPMENT_API_ENDPOINT, {
					id: equipmentId,
					name,
					equipmentTypeId,
					serialNumber,
					inventoryNumber,
					invoiceNumber,
					purchasePrice: Number(purchasePrice),
					purchasePriceUsd,
					purchaseCurrency,
					purchaseDate: getDateAsResponseType(purchaseDate as Dayjs | string),
					purchasePlace,
					guaranteeDate: getDateAsResponseType(guaranteeDate as Dayjs | string),
					characteristics,
					approverId,
					location,
					comment,
					activeHolder: activeHolderRequest,
					brokenStatus,
					jiraTask,
					storage,
					repairPlace,
				});
				toast.open(createToast(NotificationType.SUCCESS, EQUIPMENT_UPDATED));

				return data;
			} catch (error) {
				switch ((error as any).response.data.Message) {
					case SAME_SERIAL_NUMBER_ALREADY_EXISTS:
						toast.open(
							createToast(NotificationType.ERROR, PLEASE_CHANGE_SERIAL_NUMBER)
						);
						setIsSerialNumberUnique(false);
						break;
					case SAME_INVENTORY_NUMBER_ALREADY_EXISTS:
						toast.open(
							createToast(
								NotificationType.ERROR,
								PLEASE_CHANGE_INVENTORY_NUMBER
							)
						);
						setIsInventoryNumberUnique(false);
						break;
					default:
						toast.open(
							createToast(
								NotificationType.ERROR,
								(error as any).response.data.Message
							)
						);
						break;
				}

				return Promise.reject(error);
			}
		},
	});
