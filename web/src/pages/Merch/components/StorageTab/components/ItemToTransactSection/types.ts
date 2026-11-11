import { MinAmountItem } from '@/forms/MerchElementForm/types';

export interface ItemToTransactSectionProps {
	isClothing?: boolean | null;
	itemName?: string;
	minAmountSizes?: MinAmountItem[];
	transactionType?: string;
}
