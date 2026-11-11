import { PropsWithChildren } from 'react';

import { MinskSpaceCLayoutWrapper } from './styled';

export function MinskSpaceCLayout({ children }: PropsWithChildren) {
	return <MinskSpaceCLayoutWrapper>{children}</MinskSpaceCLayoutWrapper>;
}
