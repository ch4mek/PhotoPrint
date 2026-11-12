import { ReactNode } from 'react';
import { PhotoProps } from '../Photo';
export interface UserMenuProps {
    firstName: string;
    lastName: string;
    role: string;
    email: string;
    details?: string[];
    photoProps?: Partial<PhotoProps>;
    children?: ReactNode;
    dataTestId?: string;
}
