import { Roles } from '@/constants/roles';

export interface ProfileAvatarProps {
	onSubmit: (params: ProfileAvatarValues) => void;
	initialValues: ProfileAvatarValues;
}

export interface ProfileAvatarValues {
	id: number;
	departmentId: string;
	location: string;
	timeZone: string;
	avatarFileId: string | null;
	firstName: string;
	lastName: string;
	position: Roles;
	email?: string;
	picture?: string;
	message?: string;
}
