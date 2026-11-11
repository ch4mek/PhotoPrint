import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';

import { PAYLOAD_MOBILE_BREAKPOINT } from '../../contants';
import { ShowTimeToggle } from '../ShowTimeToggle';

import { PageHeader } from '@/components/PageHeader';
import { PageTitles } from '@/constants/pageTitles';
import { PayloadControls } from '@/pages/Payload/components/PayloadControls';

import { PayloadHeaderWrapper, HeaderSection } from './styles';

export function PayloadHeader() {
	const isMobileView = !useHasScreenWidth(PAYLOAD_MOBILE_BREAKPOINT);

	return (
		<PayloadHeaderWrapper>
			<HeaderSection>
				<PageHeader title={PageTitles.Payload} />
				<PayloadControls />
			</HeaderSection>
			{isMobileView && (
				<HeaderSection>
					<ShowTimeToggle />
				</HeaderSection>
			)}
		</PayloadHeaderWrapper>
	);
}
