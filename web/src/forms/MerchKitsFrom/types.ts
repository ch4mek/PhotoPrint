import {
	MerchKitCompoundPayload,
	MerchKitMinAmount,
} from '@/pages/Merch/components/KitsTab/types';
import { UpdateMerchKitPayload } from '@/queries/merchKits/types';

export interface MerchKitsFormValues {
	name: string;
	description: string | null;
	photoFileId: string | null;
	compounds: MerchKitCompoundPayload[];
	minAmounts: MerchKitMinAmount[];
	imageChangeMarker?: string | null;
}

export interface MerchKitsFormProps {
	handleClose: VoidFunction;
	handleSubmit: (values: MerchKitsFormValues) => Promise<MerchKitsFormValues>;
	handleUpdateSubmit: (
		values: UpdateMerchKitPayload
	) => Promise<MerchKitsFormValues>;
	merchId?: number | string;
}

export enum MerchKitAction {
	Archive = 'archive',
	Activate = 'activate',
}

export interface MerchKitActionConfig {
	buttonText: string;
	popUpTitle: string;
	confirmMessage: string;
	successMessage: string;
	targetStatus: string;
}
