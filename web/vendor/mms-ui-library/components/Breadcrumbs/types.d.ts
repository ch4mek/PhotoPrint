import { ReactNode } from 'react';
export type BreadcrumbsProps = {
    path: string;
    renderLink: (text: string, href?: string) => ReactNode;
    isCollapsed?: boolean;
    separator?: ReactNode;
    collapseText?: string;
    dataTestId?: string;
    className?: string;
};
