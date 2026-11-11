import { Ref } from 'react';

export interface UserCardProps {
	id?: number;
	isProfileLinkEnabled?: boolean;
	firstName: string;
	lastName: string;
	fullName?: string;
	avatarFieldId: string | null;
	position?: string | null;
	department?: string | null;
	color?: string | null;
	className?: string;
	ref?: Ref<HTMLElement | null>;
}
