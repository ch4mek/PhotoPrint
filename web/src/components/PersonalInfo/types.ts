import { ElementType, ReactNode } from 'react';

import { ProfileAvatarProps } from '../ProfileAvatar/types';

export interface PersonalInfoProps {
	isShowLoader: boolean;
	isShowSpecialization?: boolean;
	seniority: string | null;
	primarySpecialization: string | null;
	fullName: string;
	isShowComments: boolean;
	refetch: VoidFunction;
	comments: string | null;
	position?: string | null;
	isNeedWrapperStyles?: boolean;
	email?: string | null;
	avatarProps:
		| ProfileAvatarProps
		| {
				firstName: string;
				lastName: string;
				avatarFileId?: string | null;
				src?: string;
		  };
	AvatarComponent: ElementType;
	TeamsChatButtonComponent?: ReactNode;
	userId?: string | number;
	withNeedAProjectButton?: boolean;
	needProject?: boolean;
}
