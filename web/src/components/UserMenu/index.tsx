import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import axios from 'axios';
import { useState, memo, useRef } from 'react';

import { GET_BOOKING_API_ENDPOINT } from '@/constants/queryPaths';
import { useAuth } from '@/context/index';
import { useAvatar } from '@/hooks/useAvatar';
import { HEADER_ACTION_BUTTON_STYLES } from '@/layouts/main/constants';
import { ClientResponse } from '@/types/clients';

import { HEADER_AVATAR_SIZE, CUSTOM_USER_MENU_PROPS } from './constants';
import { UserMenuWrapper, CustomUserMenu, UserAvatar } from './styles';
import { UserMenuDropdown } from './UserMenuDropdown';

function UserMenuInner({
	handleOpenUserProfile,
}: {
	handleOpenUserProfile: () => void;
}) {
	const { avatarFileId, firstName, lastName } = useAuth();
	const buttonRef = useRef<HTMLButtonElement | null>(null);
	const [isOpen, setIsOpen] = useState(false);

	const { imgSource } = useAvatar(avatarFileId);

	const handleMenu = () => {
		axios.get<ClientResponse>(GET_BOOKING_API_ENDPOINT);
		setIsOpen((prevIsOpen) => !prevIsOpen);
	};

	const handleClose = () => {
		setIsOpen(false);
	};

	return (
		<UserMenuWrapper>
			<ActionButton
				color="white"
				size="large"
				isActive={isOpen}
				onClick={handleMenu}
				data-testid="user-menu"
				style={HEADER_ACTION_BUTTON_STYLES}
			>
				<UserAvatar
					src={imgSource}
					firstName={firstName}
					lastName={lastName}
					size={HEADER_AVATAR_SIZE}
				/>
			</ActionButton>
			{isOpen && (
				<CustomUserMenu
					{...CUSTOM_USER_MENU_PROPS}
					anchorEl={buttonRef.current}
					open={isOpen}
					onClose={handleClose}
				>
					<UserMenuDropdown
						handleOpenUserProfile={handleOpenUserProfile}
						onClose={handleClose}
						avatarFileId={avatarFileId}
					/>
				</CustomUserMenu>
			)}
		</UserMenuWrapper>
	);
}

export const UserMenu = memo(UserMenuInner);
