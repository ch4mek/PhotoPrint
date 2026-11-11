import { CancelType } from '@/pages/Booking/types';

export interface CancelButtonProps {
	onClick: VoidFunction;
	type: CancelType;
	disabled?: boolean;
}
