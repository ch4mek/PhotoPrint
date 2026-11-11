import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { LeftHexagonLaptopIcon } from '@mms/mms-ui-library/icons/logo/LeftHexagonLaptopIcon';
import { LeftHexagonMobileIcon } from '@mms/mms-ui-library/icons/logo/LeftHexagonMobileIcon';
import { LeftHexagonTabletIcon } from '@mms/mms-ui-library/icons/logo/LeftHexagonTabletIcon';
import { RightHexagonLaptopIcon } from '@mms/mms-ui-library/icons/logo/RightHexagonLaptopIcon';
import { RightHexagonMobileIcon } from '@mms/mms-ui-library/icons/logo/RightHexagonMobileIcon';
import { RightHexagonTabletIcon } from '@mms/mms-ui-library/icons/logo/RightHexagonTabletIcon';
import { breakpoints } from '@mms/mms-ui-library/theme';

const { desktopXS, tabletXS } = breakpoints;

export const useHexagonIcons = () => {
	const isDesktopSWidth = useHasScreenWidth(desktopXS);
	const isTabletWidth = useHasScreenWidth(tabletXS);

	if (isDesktopSWidth) {
		return [LeftHexagonLaptopIcon, RightHexagonLaptopIcon];
	}

	if (isTabletWidth) {
		return [LeftHexagonTabletIcon, RightHexagonTabletIcon];
	}

	return [LeftHexagonMobileIcon, RightHexagonMobileIcon];
};
