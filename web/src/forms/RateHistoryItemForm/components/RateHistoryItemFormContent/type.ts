import { FormAutoselectOption } from '@/types/form';

export interface RateHistoryItemFormContentProps {
	onClose: VoidFunction;
	popUpTitle: string;
	isLoading: boolean;
	handleOutsideClose: VoidFunction;
	currencyOptions: FormAutoselectOption<string>[];
}
