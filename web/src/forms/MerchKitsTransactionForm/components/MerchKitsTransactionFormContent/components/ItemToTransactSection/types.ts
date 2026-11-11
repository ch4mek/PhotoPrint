import { SizeCount } from '@/forms/MerchKitsTransactionForm/types';

export interface ItemToTransactSectionProps {
	itemName?: string;
	sizes: SizeCount[];
	isReciept: boolean;
}
