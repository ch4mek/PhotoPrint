import { MerchElementsFormValues } from '@/forms/MerchElementForm/types';
import { MerchSizeType, MerchStorageType } from '@/types/configuration';

export interface SizesInputsSectionProps {
	merchSizes: MerchSizeType[];
	merchStorages: MerchStorageType[];
	initialFormData: MerchElementsFormValues;
	disabled?: boolean;
}
