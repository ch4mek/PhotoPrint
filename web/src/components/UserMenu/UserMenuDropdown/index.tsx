import { useTheme } from '@emotion/react';
import { UserMenu } from '@mms/mms-ui-library/components/UserMenu';
import { UserMenuItem } from '@mms/mms-ui-library/components/UserMenuItem';
import { memo, useCallback } from 'react';

import { LogoutReasons } from '@/constants/authConstants';
import { useAuth, useColorMode } from '@/context/index';
import { useAvatar } from '@/hooks/useAvatar';
import { ThemeMode } from '@/theme';

import { timezoneDetails, menuItemsProps } from './constants';
import type { DropdownProps } from './types';

function UserMenuDropdownInner({
	avatarFileId,
	onClose,
	handleOpenUserProfile,
}: DropdownProps) {
	const { lastName, firstName, rightPosition, email, onLogout } = useAuth();

	const { imgSource } = useAvatar(avatarFileId);

	const { toggleThemeMode } = useColorMode();
	const theme = useTheme();

	const handleProfileEdit = useCallback(() => {
		onClose();
		handleOpenUserProfile();
	}, [onClose]);

	const handleLogOut = useCallback(
		() => onLogout(LogoutReasons.VIA_BUTTON),
		[onLogout]
	);

	return (
		<UserMenu
			firstName={firstName}
			lastName={lastName}
			email={email}
			role={rightPosition}
			details={timezoneDetails}
			photoProps={{ src: imgSource, bgColor: theme.palette['hover-primary-1'] }}
		>
			<UserMenuItem {...menuItemsProps.profile} onClick={handleProfileEdit} />
			<UserMenuItem
				{...(theme.palette.mode === ThemeMode.DARK
					? menuItemsProps.lightTheme
					: menuItemsProps.darkTheme)}
				onClick={toggleThemeMode}
			/>
			<UserMenuItem {...menuItemsProps.logOut} onClick={handleLogOut} />
		</UserMenu>
	);
}

export const UserMenuDropdown = memo(UserMenuDropdownInner);
