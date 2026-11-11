import {
	ASSIGNED_EQUIPMENT_STATUS_OPTIONS,
	StatusEquipment,
} from '../constants';

import { Currency } from '@/constants/currency';
import { findById } from '@/helpers/findById';
import dayjs from '@/lib/dayjs';
import {
	EquipmentAvailabilityType,
	EquipmentBrokenStatus,
	EquipmentType,
} from '@/types/configuration';
import { EquipmentItemResponse } from '@/types/equipment';

import { MainEquipmentFormValues } from './types';

export const convertEquipmentResponseToFormValues = (
	{
		guaranteeDate,
		purchaseDate,
		purchasePrice,
		purchasePriceUsd,
		purchasePriceUsdCurrency = Currency.USD,
		location,
		equipmentTypeId,
		...restProps
	}: EquipmentItemResponse,
	equipmentTypes: Array<EquipmentType>
): MainEquipmentFormValues => ({
	...restProps,
	guaranteeDate: dayjs(guaranteeDate),
	purchaseDate: dayjs(purchaseDate),
	purchasePrice: String(purchasePrice),
	purchasePriceUsd: String(purchasePriceUsd),
	purchasePriceUsdCurrency,
	equipmentType: findById(equipmentTypes, equipmentTypeId) || null,
	location: {
		id: location,
		name: location,
		description: location,
		location: null,
	},
});

export const convertStatusToRequestBrokenStatus = (
	brokenStatuses: EquipmentBrokenStatus[],
	status: string
) =>
	brokenStatuses.find((item) => item.description === status)?.id ||
	StatusEquipment.NONE;

export const getStatusOptions = (
	equipmentAvailabilityTypes: EquipmentAvailabilityType[],
	isEquipmentAssigned: boolean
) =>
	equipmentAvailabilityTypes
		.map(({ description }) => description)
		.filter((option) => {
			if (isEquipmentAssigned) {
				return ASSIGNED_EQUIPMENT_STATUS_OPTIONS.includes(option);
			}

			return option !== StatusEquipment.ISSUED;
		});
