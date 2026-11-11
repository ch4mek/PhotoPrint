import { ByPageResponse } from '../types';

import { UserSpecializationInfo } from '@/types/userProfile';

export type RequestData = {
	id: number;
	firstName: string;
	lastName: string;
	timeZone: string;
	avatarFileId: string | null;
	departmentId: string;
	location: string;
	message?: string;
};

export interface UpdateUserLocationPayload {
	userId: number;
	location: string;
}

export type UserHardSkillsResponse = ByPageResponse<UserSpecializationInfo>;
