import { DiscountSubmitData } from '@/types/discounts';

export interface DiscountFormProps {
	handleClose: VoidFunction;
	handleSubmit: (values: DiscountSubmitData) => Promise<void>;
	discountId?: number | string;
}
