import { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';

import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { useShowModal } from '@/pages/hooks/useShowModal';

export function useShowUserProfile() {
	const { handleOpen, isOpen, handleClose, id } = useShowModal();
	const [searchParams, setSearchParams] = useSearchParams();

	useEffect(() => {
		const userProfileTabId = searchParams.get(USER_PROFILE_TAB_ID_PARAM);

		if (userProfileTabId) {
			handleOpen();
		}
	}, [searchParams]);

	const handleCloseUserProfile = () => {
		const userProfileTabId = searchParams.get(USER_PROFILE_TAB_ID_PARAM);

		if (userProfileTabId) {
			setSearchParams();
		}

		handleClose();
	};

	return { id, isOpen, handleOpen, handleClose: handleCloseUserProfile };
}
