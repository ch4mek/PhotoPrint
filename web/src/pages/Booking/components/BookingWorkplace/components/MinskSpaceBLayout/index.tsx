import { PropsWithChildren } from 'react';

import { MinskSpaceBLayoutWrapper } from './styled';

export function MinskSpaceBLayout({ children }: PropsWithChildren) {
	return <MinskSpaceBLayoutWrapper>{children}</MinskSpaceBLayoutWrapper>;
}
