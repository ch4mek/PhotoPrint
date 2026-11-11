import { useCallback } from 'react';

import { useAuth } from '@/context/index';
import { personalInfoPopupAccessRules } from '@/helpers/personalInfoPopupAccessRules';

export const usePersonalInfoPopupGuard = () => {
	const { id: currentUserId } = useAuth();

	return useCallback(
		(userId: number) =>
			personalInfoPopupAccessRules.notOwnProfile(userId, currentUserId),
		[currentUserId]
	);
};
