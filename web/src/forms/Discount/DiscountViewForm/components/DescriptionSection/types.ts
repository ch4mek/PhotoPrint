import { Discount } from '@/types/discounts';
import { CustomRootBlock } from '@/types/plateEditor';

export interface DescriptionSectionProps {
	initialData: CustomRootBlock[];
	discountData?: Discount;
	initialText?: string;
}
