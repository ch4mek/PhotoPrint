import { JSX } from 'react';
type Status = 'error' | 'info' | 'success';
export interface NewToastProps {
    status: Status;
    text: JSX.Element | string;
    onClose: () => void;
    dataTestId?: string;
}
export interface StatusProps {
    status: Status;
}
export {};
