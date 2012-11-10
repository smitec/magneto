% make a trapezoid
% rise time, high time, fall time in ms, sample rate in hz

function out = trapgen(sample_rate, rise_time, high_time, fall_time)
    out = linspace(0,1, rise_time/(1000/sample_rate));
    out = [out ones(1,high_time/(1000/sample_rate))];
    out = [out linspace(1,0, fall_time/(1000/sample_rate))];
end