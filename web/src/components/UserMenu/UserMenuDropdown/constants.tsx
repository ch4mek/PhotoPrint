import { LightModeIcon } from '@mms/mms-ui-library/icons/base/LightModeIcon';
import { NightModeIcon } from '@mms/mms-ui-library/icons/base/NightModeIcon';
import { ProfileIcon } from '@mms/mms-ui-library/icons/base/ProfileIcon';
import { SignOutIcon } from '@mms/mms-ui-library/icons/base/SignOutIcon';

import { localTime, timezoneName } from './utils';

export const menuItemsProps = {
	profile: {
		title: 'Profile',
		icon: <ProfileIcon />,
	},
	lightTheme: {
		title: 'Light theme',
		icon: <LightModeIcon />,
	},
	darkTheme: {
		title: 'Dark theme',
		icon: <NightModeIcon />,
	},
	logOut: {
		title: 'Log out',
		icon: <SignOutIcon />,
	},
};

export const timezoneDetails = [`Timezone: ${localTime}`, timezoneName];
