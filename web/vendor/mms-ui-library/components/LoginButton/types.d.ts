import { ButtonHTMLAttributes, PropsWithChildren, ReactNode, RefAttributes } from 'react';
export interface LoginButtonProps extends PropsWithChildren, ButtonHTMLAttributes<HTMLButtonElement>, RefAttributes<HTMLButtonElement> {
    leftAddon: ReactNode;
    dataTestId?: string;
}
