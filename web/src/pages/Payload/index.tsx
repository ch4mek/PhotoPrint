import { PayloadProvider } from '@/context/PayloadContext';

import { PayloadGridWrapper } from './components/PayloadGridWrapper';
import { PayloadHeader } from './components/PayloadHeader';
import { PayloadQuickFilters } from './components/PayloadQuickFilters';
import { Wrapper } from './styles';

export function PayloadPage() {
	return (
		<Wrapper>
			<PayloadProvider>
				<PayloadHeader />
				<PayloadQuickFilters />
				<PayloadGridWrapper />
			</PayloadProvider>
		</Wrapper>
	);
}
