import { TeamboardUserProfileKind } from '../types';

import { isIntern, Roles } from '@/constants/roles';

export const getProfileKind = (position?: Roles) =>
	position != null && isIntern(position)
		? TeamboardUserProfileKind.Intern
		: TeamboardUserProfileKind.Employee;
