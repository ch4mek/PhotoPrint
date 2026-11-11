import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';

import {
	FULL_NAME_BREAKPOINT,
	APP_FULL_NAME,
	APP_SHORT_NAME,
} from './constants';

export const getTitleLogoText = () => {
	const shouldDisplayFullName = useHasScreenWidth(FULL_NAME_BREAKPOINT);

	return shouldDisplayFullName ? APP_FULL_NAME : APP_SHORT_NAME;
};
