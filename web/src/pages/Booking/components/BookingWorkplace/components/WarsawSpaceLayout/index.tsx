import { PropsWithChildren } from 'react';

import { WarsawSpaceLayoutWrapper } from './styled';

export function WarsawSpaceLayout({ children }: PropsWithChildren) {
	return <WarsawSpaceLayoutWrapper>{children}</WarsawSpaceLayoutWrapper>;
}
