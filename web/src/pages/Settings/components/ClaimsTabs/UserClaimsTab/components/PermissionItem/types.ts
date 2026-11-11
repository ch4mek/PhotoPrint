import { Dispatch, SetStateAction } from 'react';

import { UserClaimsGroup } from '@/pages/Settings/components/ClaimsTabs/types';

export interface PermissionItemProps {
	item: UserClaimsGroup;
	permissions: string[];
	setActivePermissions: Dispatch<SetStateAction<string[]>>;
}
