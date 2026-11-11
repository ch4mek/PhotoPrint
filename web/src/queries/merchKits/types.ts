import {
	MerchKitCompoundPayload,
	MerchKitMinAmount,
} from '@/pages/Merch/components/KitsTab/types';

export interface UpdateMerchKitStatusPayload {
	id: number | string;
	status: string;
}

export interface UpdateMerchKitStatusResponse {
	id: number;
	name: string;
	status: string;
}

export interface UpdateMerchKitPayload {
	id?: number | string;
	name: string;
	photoFileId: string | null;
	description: string | null;
	compounds: MerchKitCompoundPayload[];
	minAmounts: MerchKitMinAmount[];
}
