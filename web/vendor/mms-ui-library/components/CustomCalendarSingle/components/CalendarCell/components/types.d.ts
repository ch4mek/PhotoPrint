import { HTMLAttributes, ReactElement, Ref } from 'react';
import { Holiday } from '@/components/CustomCalendarSingle/types';
export interface CalendarCellWrapperProps {
    holiday?: Holiday;
    children: ReactElement<{
        ref?: Ref<HTMLElement>;
    } & HTMLAttributes<HTMLElement>>;
}
