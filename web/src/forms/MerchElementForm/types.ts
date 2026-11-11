export interface MerchElementsFormValues {
	id?: number | string;
	name: string;
	supplier: string | null;
	description: string | null;
	photoFileId: string | null;
	imageChangeMarker?: string | null;
	isClothing: boolean | null;
	sizeMinAmounts: MinAmountItem[];
}

export interface MinAmountItem {
	sizeId: string;
	storageId: number;
	minAmount: number;
	amount: number;
}

export interface MerchElementUpdateStatus {
	status: MerchElementStatus;
}

export enum MerchElementStatus {
	Available = 'Available',
	Archived = 'Archived',
}

export interface MerchElementsFormProps {
	handleClose: VoidFunction;
	handleSubmit: (
		values: MerchElementsFormValues
	) => Promise<MerchElementsFormValues>;
	handleUpdateSubmit: (
		values: MerchElementsFormValues
	) => Promise<MerchElementsFormValues>;
	handleUpdateStatus: (
		values: MerchElementUpdateStatus,
		successMessage: string
	) => Promise<MerchElementUpdateStatus>;
	merchId?: number | string;
	isLoading?: boolean;
	isLoadingStatus?: boolean;
}

export interface ValueByMerchElementStatus<T> {
	active: T;
	archived: T;
	archivedWithKit: T;
}

export interface GetValueByMerchElementStatusParams<T> {
	isArchived: boolean;
	isPartOfAvailableKit?: boolean | null;
	valueByStatus: ValueByMerchElementStatus<T>;
}

export interface ConfirmationModalTextValue {
	title: string;
	confirmationMessage: string;
	contentText?: string;
}
