import { UserInquiryApprover, AllRolesOptions } from '@/types/absences';

export const convertDataToUserCard = (
	item?: UserInquiryApprover | null,
	currentUserId?: number,
	acceptUserPosition?: string,
	acceptPolicyScope?: AllRolesOptions
) => {
	if (!item) {
		return null;
	}

	const { firstName, lastName, avatarFieldId, id } = item;

	return {
		id,
		firstName,
		lastName,
		avatarFieldId,
		position:
			acceptPolicyScope === AllRolesOptions.PM
				? acceptPolicyScope
				: acceptUserPosition ?? item.position,
		showProfileLink:
			item.canUserInfoBeRetrievedByCurrentUser && currentUserId !== id,
	};
};
