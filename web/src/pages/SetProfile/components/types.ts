import { Roles } from '@/constants/roles';

export interface ProfileFormProps {
	onSubmit: (params: ProfileFormValues) => void;
	isSubmitting: boolean;
}

export interface ProfileFormValues {
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
}
