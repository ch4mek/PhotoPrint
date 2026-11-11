import { Dispatch, RefObject, SetStateAction } from 'react';

import { EquipmentTabs, StatusEquipment } from '../constants';
import type { EquipmentFormProps } from '../types';

import { Currency } from '@/constants/currency';
import { OutsideCloseHandler } from '@/forms/components/FormikWithBackup/types';
import type { Dayjs } from '@/lib/dayjs';
import {
	EquipmentType,
	LocationType,
	LocationTypeId,
} from '@/types/configuration';
import { ActiveHolder, Approver } from '@/types/equipment';

export type MainEquipmentFormProps = Pick<
	EquipmentFormProps,
	'onClose' | 'id'
> & {
	setEquipmentId: Dispatch<SetStateAction<string>>;
	setIsFormValid: Dispatch<SetStateAction<boolean>>;
	setActiveTab: Dispatch<SetStateAction<EquipmentTabs>>;
	onLoad: Dispatch<SetStateAction<boolean>>;
	submitFormButtonRef: RefObject<HTMLButtonElement | null>;
	outsideCloseRef: RefObject<OutsideCloseHandler | null>;
	setStatus: (value: StatusEquipment) => void;
};

export interface MainEquipmentFormValues {
	name: string;
	activeHolder: ActiveHolder | null;
	equipmentType: EquipmentType | null;
	invoiceNumber: string;
	serialNumber: string;
	inventoryNumber: string;
	purchaseDate: Dayjs;
	purchasePrice: string;
	purchasePriceUsd: string;
	purchasePlace: string;
	purchaseCurrency: Currency;
	purchasePriceUsdCurrency: Currency;
	guaranteeDate: Dayjs;
	approver: Approver | null;
	storage: LocationTypeId | null;
	location: LocationType | null;
	characteristics: string;
	jiraTask: string;
	comment: string;
	status: StatusEquipment;
	repairPlace: string;
}
