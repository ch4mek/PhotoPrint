import { Roles } from '@/constants/roles';
import { OfficeType } from '@/types/configuration';

export interface ItemInnerProps {
	currentUserId: number;
	userRole: Roles;
	userId: number;
	isBlock: boolean;
	lastName: string;
	firstName: string;
	weeksInInterval: number;
	dateList: string[];
	workplaceNumber: number;
	office: OfficeType;
	workspaceId: number | null;
	handleDelete: VoidFunction;
	handleClickName: (
		userId: string | number,
		isNeedOpenInfo: boolean
	) => VoidFunction;
	isNameClickable: boolean;
	isForGuest: boolean;
	guestName: string | null;
	bookedBy?: string | null;
}
