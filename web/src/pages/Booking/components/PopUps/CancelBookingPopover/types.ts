import { MouseEvent, RefAttributes } from 'react';

import { CancelType } from '@/pages/Booking/types';

export interface CancelBookingPopoverProps
	extends RefAttributes<HTMLDivElement> {
	isOpen: boolean;
	type: CancelType;
	spaceInfo: string;
	bookedDate: string | null;
	handleClose: VoidFunction;
	handleSubmit: VoidFunction;
	handleInfoClick?: VoidFunction;
	bookedDateTime?: string | null;
	bookedUserInfo?: string | null;
	distanceToAnchorElement?: number;
	onWrapperClick?: (e: MouseEvent<HTMLDivElement>) => void;
}
