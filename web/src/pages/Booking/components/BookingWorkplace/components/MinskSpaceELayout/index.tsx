import { PropsWithChildren } from 'react';

import { MinskSpaceELayoutWrapper } from './styled';

export function MinskSpaceELayout({ children }: PropsWithChildren) {
	return <MinskSpaceELayoutWrapper>{children}</MinskSpaceELayoutWrapper>;
}
