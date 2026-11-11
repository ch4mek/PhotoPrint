import { PropsWithChildren } from 'react';

import { MinskSpaceALayoutWrapper } from './styled';

export function MinskSpaceALayout({ children }: PropsWithChildren) {
	return <MinskSpaceALayoutWrapper>{children}</MinskSpaceALayoutWrapper>;
}
