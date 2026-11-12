import { JSX } from 'react';
type Status = 'error' | 'info' | 'success';
export interface ToastProps {
    status: Status;
    title: JSX.Element | null | string;
    text: JSX.Element | string;
    onClose: () => void;
    dataTestId?: string;
}
export interface StatusProps {
    status: Status;
}
export {};
