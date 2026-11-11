import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { ReactNode } from 'react';

import { DetailedInfoList, DetailedInfoScrollWrapper } from './styles';

export function DetailedInfoContainer({ children }: { children: ReactNode }) {
	const isTabletXSWidth = useHasScreenWidth(breakpoints.tabletXS);

	return (
		<DetailedInfoScrollWrapper
			scroll={isTabletXSWidth ? 'vertical' : 'horizontal'}
		>
			<DetailedInfoList>{children}</DetailedInfoList>
		</DetailedInfoScrollWrapper>
	);
}
