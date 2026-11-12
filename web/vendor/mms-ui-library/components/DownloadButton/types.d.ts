import { ButtonHTMLAttributes, ReactNode, Ref } from 'react';
export interface DownloadButtonWrapperProps {
    dataTestId?: string;
    ref?: Ref<HTMLButtonElement>;
}
export type DownloadButtonProps = DownloadButtonWrapperProps & ButtonHTMLAttributes<HTMLButtonElement> & {
    icon?: ReactNode;
};
