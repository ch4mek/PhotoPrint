import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import {
	isManagerRoleOrGroupManager,
	isNotChiefRole,
	Roles,
} from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth } from '@/context/AuthContext';
import { useBookingContext } from '@/context/Booking';

import { usePersonalInfoPopupGuard } from './usePersonalInfoPopupGuard';

interface OpenPersonalInfoParams {
	onClose: VoidFunction;
	userId?: number | string;
	isGuest?: boolean;
}

export const useOpenPersonalInfo = ({
	onClose,
	userId,
	isGuest = false,
}: OpenPersonalInfoParams) => {
	const { role } = useAuth();
	const { bookingUsersData } = useBookingContext();

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const canOpen = Boolean(
		userId &&
			!isGuest &&
			canOpenProfile(+userId) &&
			isManagerRoleOrGroupManager(role) &&
			isNotChiefRole(
				bookingUsersData?.find((item) => item.id === userId)
					?.positionDescription as Roles
			)
	);

	const handleClickName = () => {
		if (!userId) {
			return;
		}
		handlePageOpen(userId.toString());
		onClose();
	};

	return { handleClickName: canOpen ? handleClickName : undefined };
};
