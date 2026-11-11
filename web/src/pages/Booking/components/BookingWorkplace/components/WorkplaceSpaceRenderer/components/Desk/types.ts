import { DeskInfo, OfficeDeskPosition } from '../../../../types';

import { COLORS_ENUM } from '@/constants/booking';
import { DeskSize } from '@/types/booking';
import { OfficeType } from '@/types/configuration';
import { WithMobileView } from '@/types/filter';
import { UserInfo } from '@/types/userInfo';

export interface DeskInnerProps extends OfficeDeskPosition, WithMobileView {
	office: OfficeType;
	isCanCanceled: boolean;
	handleCancelClick: VoidFunction;
	workspaceId: number | null;
	firstName?: string;
	lastName?: string;
	userId?: number;
	isBlocked?: boolean;
	currentUser?: UserInfo;
	isSelected?: boolean;
	department?: string | null;
	onClick: (deskInfo: DeskInfo | null) => void;
	size: DeskSize;
	isForGuest?: boolean;
	guestName?: string | null;
	bookedBy?: string | null;
	rotate?: number;
	dateList?: Array<string>;
	weeksInInterval?: number;
	workplaceNumber?: number;
}

export interface DeskTooltipTextWrapperProps {
	isMobile: boolean;
}

export type ColorType = (typeof COLORS_ENUM)[keyof typeof COLORS_ENUM];

interface CommonDeskElementProps {
	height: number;
	width: number;
}

export interface WorkplaceProps extends CommonDeskElementProps {
	isBlocked: boolean;
	isBooked: boolean;
	isBookedByUser: boolean;
	isSelected: boolean;
	isDisabled: boolean;
	isForGuest: boolean;
	rotate?: number;
}
