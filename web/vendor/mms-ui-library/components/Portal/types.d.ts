import { ReactNode, RefObject } from 'react';
export interface PortalProps {
    children?: ReactNode;
    getPortalContainer?: () => Element | null;
    immediateMount?: boolean;
    ref?: RefObject<Element>;
}
